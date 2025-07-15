from odoo import models, fields, api


class FormFlowTemplate(models.Model):
    _name = 'formflow.template'
    _description = 'FormFlow Template'
    _order = 'create_date desc'

    name = fields.Char(string='Title', required=True)
    description = fields.Text(string='Description')
    author_name = fields.Char(string='Author', required=True)
    template_id = fields.Char(string='Template ID', required=True)
    is_published = fields.Boolean(string='Published', default=False)
    total_responses = fields.Integer(string='Total Responses', default=0)
    created_at = fields.Datetime(string='Created At')
    
    question_ids = fields.One2many('formflow.question', 'template_id', string='Questions')
    aggregated_result_ids = fields.One2many('formflow.aggregated.result', 'template_id', string='Aggregated Results')
    
    api_token = fields.Char(string='API Token')
    last_import_date = fields.Datetime(string='Last Import Date')
    
    def action_refresh_data(self):
        self._import_aggregated_data(self.api_token, "https://www.form-flow.xyz")
        return {'type': 'ir.actions.client', 'tag': 'reload'}

    @api.model
    def import_from_api(self, api_token, api_url):
        import requests
        import json
        from datetime import datetime
        
        headers = {'X-API-Token': api_token}
        
        try:
            response = requests.get(f"{api_url}/api/odoo/templates", headers=headers, timeout=30)
            response.raise_for_status()
            templates_data = response.json()
            
            for template_data in templates_data:
                existing = self.search([('template_id', '=', template_data['id'])])
                
                vals = {
                    'name': template_data['title'],
                    'description': template_data['description'],
                    'author_name': template_data['author'],
                    'template_id': template_data['id'],
                    'is_published': template_data['isPublished'],
                    'total_responses': template_data['totalResponses'],
                    'created_at': datetime.fromisoformat(template_data['createdAt'].replace('Z', '+00:00')),
                    'api_token': api_token,
                    'last_import_date': fields.Datetime.now(),
                }
                
                if existing:
                    existing.write(vals)
                    template_record = existing
                else:
                    template_record = self.create(vals)
                
                template_record.question_ids.unlink()
                
                for question_data in template_data.get('questions', []):
                    self.env['formflow.question'].create({
                        'template_id': template_record.id,
                        'question_id': question_data['id'],
                        'title': question_data['title'],
                        'description': question_data['description'],
                        'question_type': question_data['type'],
                        'order': question_data['order'],
                        'is_required': question_data['isRequired'],
                    })
                
                template_record._import_aggregated_data(api_token, api_url)
            
            return len(templates_data)
            
        except Exception as e:
            raise Exception(f"Failed to import templates: {str(e)}")
    
    
    def _import_aggregated_data(self, api_token, api_url):
        import requests
        
        headers = {'X-API-Token': api_token}
        
        try:
            url = f"{api_url}/api/odoo/templates/{self.template_id}/aggregated"
            response = requests.get(url, headers=headers, timeout=30)
            
            if response.status_code == 200:
                data = response.json()
                
                self.aggregated_result_ids.unlink()
                
                for question_data in data.get('questions', []):
                    self.env['formflow.aggregated.result'].create_from_api_data(self.id, question_data)
                    
        except Exception:
            pass
