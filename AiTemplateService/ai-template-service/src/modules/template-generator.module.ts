import { Module } from '@nestjs/common';
import { OpenAIService } from 'src/services/openai.service';
import { TemplateGeneratorController } from './template-generator.controller';

@Module({
    controllers: [TemplateGeneratorController],
    providers: [OpenAIService],
})
export class TemplateGeneratorModule {}
