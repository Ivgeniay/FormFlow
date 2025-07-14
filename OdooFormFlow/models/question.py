from odoo import models, fields


class FormFlowQuestion(models.Model):
    _name = 'formflow.question'
    _description = 'FormFlow Question'
    _order = 'order asc'

    template_id = fields.Many2one('formflow.template', string='Template', required=True, ondelete='cascade')
    question_id = fields.Char(string='Question ID', required=True)
    title = fields.Char(string='Title', required=True)
    description = fields.Text(string='Description')
    question_type = fields.Char(string='Type', required=True)
    order = fields.Integer(string='Order', default=1)
    is_required = fields.Boolean(string='Required', default=False)
    
    _sql_constraints = [
        ('unique_question_template', 'UNIQUE(template_id, question_id)', 'Question ID must be unique per template.')
    ]