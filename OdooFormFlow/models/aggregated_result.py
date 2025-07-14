from odoo import models, fields


class FormFlowAggregatedResult(models.Model):
    _name = 'formflow.aggregated.result'
    _description = 'FormFlow Aggregated Result'

    template_id = fields.Many2one('formflow.template', string='Template', required=True, ondelete='cascade')
    question_id = fields.Char(string='Question ID', required=True)
    question_title = fields.Char(string='Question Title', required=True)
    question_type = fields.Char(string='Question Type', required=True)
    aggregated_data = fields.Text(string='Aggregated Data')
    total_answers = fields.Integer(string='Total Answers', default=0)
    
    average_value = fields.Float(string='Average Value')
    min_value = fields.Float(string='Min Value')
    max_value = fields.Float(string='Max Value')
    most_popular_answer = fields.Char(string='Most Popular Answer')
    
    _sql_constraints = [
        ('unique_question_result', 'UNIQUE(template_id, question_id)', 'Aggregated result must be unique per question.')
    ]