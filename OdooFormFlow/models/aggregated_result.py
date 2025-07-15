from odoo import models, fields, api
import json


class FormFlowAggregatedResult(models.Model):
    _name = 'formflow.aggregated.result'
    _description = 'FormFlow Aggregated Result'

    template_id = fields.Many2one('formflow.template', string='Template', required=True, ondelete='cascade')
    question_id = fields.Char(string='Question ID', required=True)
    question_title = fields.Char(string='Question Title', required=True)
    question_type = fields.Char(string='Question Type', required=True)
    aggregated_data = fields.Text(string='Aggregated Data')
    total_answers = fields.Integer(string='Total Answers', default=0)
    
    average_value = fields.Float(string='Average')
    min_value = fields.Float(string='Min Value')
    max_value = fields.Float(string='Max Value')
    
    most_popular_answer = fields.Char(string='Most Popular Answer')
    most_popular_count = fields.Integer(string='Most Popular Count')
    
    earliest_date = fields.Char(string='Earliest Date')
    latest_date = fields.Char(string='Latest Date')
    
    most_popular_time = fields.Char(string='Most Popular Time')
    most_popular_time_count = fields.Integer(string='Most Popular Time Count')
    
    option_counts = fields.One2many('formflow.option.count', 'result_id', string='Option Counts')
    popular_answers = fields.One2many('formflow.popular.answer', 'result_id', string='Popular Answers')
    
    @api.model
    def create_from_api_data(self, template_id, question_data):
        aggregated_results = question_data.get('aggregatedResults', {})
        question_type = question_data.get('type', '').lower()
        
        vals = {
            'template_id': template_id,
            'question_id': question_data['questionId'],
            'question_title': question_data['title'],
            'question_type': question_data['type'],
            'aggregated_data': json.dumps(aggregated_results),
            'total_answers': aggregated_results.get('totalAnswers', 0),
        }
        
        if question_type in ['scale', 'rating']:
            vals.update({
                'average_value': aggregated_results.get('average', 0),
                'min_value': aggregated_results.get('min', 0),
                'max_value': aggregated_results.get('max', 0),
            })
        
        elif question_type in ['shorttext', 'longtext']:
            popular_answers = aggregated_results.get('mostPopularAnswers', [])
            if popular_answers:
                vals.update({
                    'most_popular_answer': popular_answers[0].get('answer', ''),
                    'most_popular_count': popular_answers[0].get('count', 0),
                })
        
        elif question_type in ['singlechoice', 'dropdown']:
            option_counts = aggregated_results.get('optionCounts', [])
            if option_counts:
                vals.update({
                    'most_popular_answer': option_counts[0].get('option', ''),
                    'most_popular_count': option_counts[0].get('count', 0),
                })
        
        elif question_type == 'date':
            vals.update({
                'earliest_date': aggregated_results.get('earliestDate', ''),
                'latest_date': aggregated_results.get('latestDate', ''),
            })
        
        elif question_type == 'time':
            popular_times = aggregated_results.get('mostPopularTimes', [])
            if popular_times:
                vals.update({
                    'most_popular_time': popular_times[0].get('time', ''),
                    'most_popular_time_count': popular_times[0].get('count', 0),
                })
        
        result = self.create(vals)
        
        if question_type in ['shorttext', 'longtext']:
            for answer_data in aggregated_results.get('mostPopularAnswers', []):
                self.env['formflow.popular.answer'].create({
                    'result_id': result.id,
                    'answer': answer_data.get('answer', ''),
                    'count': answer_data.get('count', 0),
                })
        
        elif question_type in ['singlechoice', 'dropdown', 'multiplechoice']:
            for option_data in aggregated_results.get('optionCounts', []):
                self.env['formflow.option.count'].create({
                    'result_id': result.id,
                    'option': option_data.get('option', ''),
                    'count': option_data.get('count', 0),
                })
        
        elif question_type == 'time':
            for time_data in aggregated_results.get('mostPopularTimes', []):
                self.env['formflow.popular.answer'].create({
                    'result_id': result.id,
                    'answer': time_data.get('time', ''),
                    'count': time_data.get('count', 0),
                })
        
        return result
    
    _sql_constraints = [
        ('unique_question_result', 'UNIQUE(template_id, question_id)', 'Aggregated result must be unique per question.')
    ]


class FormFlowOptionCount(models.Model):
    _name = 'formflow.option.count'
    _description = 'FormFlow Option Count'
    _order = 'count desc'

    result_id = fields.Many2one('formflow.aggregated.result', string='Result', required=True, ondelete='cascade')
    option = fields.Char(string='Option', required=True)
    count = fields.Integer(string='Count', required=True)


class FormFlowPopularAnswer(models.Model):
    _name = 'formflow.popular.answer'
    _description = 'FormFlow Popular Answer'
    _order = 'count desc'

    result_id = fields.Many2one('formflow.aggregated.result', string='Result', required=True, ondelete='cascade')
    answer = fields.Char(string='Answer', required=True)
    count = fields.Integer(string='Count', required=True)