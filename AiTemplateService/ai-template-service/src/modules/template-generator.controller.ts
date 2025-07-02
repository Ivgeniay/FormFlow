import {
    Controller,
    Post,
    Body,
    Headers,
    HttpException,
    HttpStatus,
} from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse } from '@nestjs/swagger';
import { OpenAIService } from 'src/services/openai.service';
import { GeneratedTemplate } from 'src/types/template.types';

@ApiTags('Template Generator')
@Controller('api/template-generator')
export class TemplateGeneratorController {
    constructor(private readonly openAIService: OpenAIService) {}

    @Post('generate')
    @ApiOperation({ summary: 'Generate template using AI' })
    @ApiResponse({
        status: 200,
        description: 'Template generated successfully',
    })
    @ApiResponse({ status: 400, description: 'Invalid request' })
    async generateTemplate(
        @Body() body: { prompt: string },
    ): Promise<GeneratedTemplate> {
        if (!body.prompt || body.prompt.trim().length === 0) {
            throw new HttpException(
                'Prompt is required',
                HttpStatus.BAD_REQUEST,
            );
        }

        try {
            return await this.openAIService.generateTemplate(
                body.prompt.trim(),
            );
        } catch (error) {
            throw new HttpException(
                `Failed to generate template: ${error}`,
                HttpStatus.INTERNAL_SERVER_ERROR,
            );
        }
    }
}
