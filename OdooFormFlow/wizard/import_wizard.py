from odoo import models, fields, api, _
from odoo.exceptions import UserError


class FormFlowImportWizard(models.TransientModel):
    _name = 'formflow.import.wizard'
    _description = 'FormFlow Import Wizard'

    api_token = fields.Char(string='API Token', required=True, help='Enter your FormFlow API token')
    api_url = fields.Char(string='API URL', required=True, default='http://147.45.66.49:8080', 
                         help='FormFlow API base URL')
    
    def action_import(self):
        if not self.api_token:
            raise UserError(_('API Token is required'))
        
        if not self.api_url:
            raise UserError(_('API URL is required'))
        
        try:
            template_model = self.env['formflow.template']
            imported_count = template_model.import_from_api(self.api_token, self.api_url)
            
            return {
                'type': 'ir.actions.client',
                'tag': 'display_notification',
                'params': {
                    'title': _('Import Successful'),
                    'message': _('Successfully imported %d templates') % imported_count,
                    'type': 'success',
                    'sticky': False,
                }
            }
            
        except Exception as e:
            raise UserError(_('Import failed: %s') % str(e))
    
    def action_test_connection(self):
        if not self.api_token or not self.api_url:
            raise UserError(_('Please fill in both API Token and API URL'))
        
        try:
            import requests
            headers = {'X-API-Token': self.api_token}
            response = requests.get(f"{self.api_url}/api/odoo/templates", headers=headers, timeout=10)
            
            if response.status_code == 200:
                return {
                    'type': 'ir.actions.client',
                    'tag': 'display_notification',
                    'params': {
                        'title': _('Connection Successful'),
                        'message': _('API connection is working correctly'),
                        'type': 'success',
                        'sticky': False,
                    }
                }
            else:
                raise UserError(_('API returned status code: %d') % response.status_code)
                
        except requests.exceptions.RequestException as e:
            raise UserError(_('Connection failed: %s') % str(e))
        except Exception as e:
            raise UserError(_('Test failed: %s') % str(e))