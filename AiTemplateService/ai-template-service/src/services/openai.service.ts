import { Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import OpenAI from 'openai';
import { GeneratedTemplate } from '../types/template.types';
import { templateSchema } from '../schemas/template.schema';

@Injectable()
export class OpenAIService {
    private openai: OpenAI;

    constructor(private configService: ConfigService) {
        this.openai = new OpenAI({
            apiKey: this.configService.get<string>('openai.apiKey'),
        });
    }

    async generateTemplate(prompt: string): Promise<GeneratedTemplate> {
        const systemPrompt = this.buildSystemPrompt();
        const userPrompt = this.buildUserPrompt(prompt);

        const ai_model = this.configService.get<string>('openai.model');
        const completion = await this.openai.chat.completions.create({
            model: `${ai_model}`,
            messages: [
                { role: 'system', content: systemPrompt },
                { role: 'user', content: userPrompt },
            ],
            seed: Date.now(),
            max_tokens: this.configService.get<number>('openai.maxTokens'),
            temperature: this.configService.get<number>('openai.temperature'),
            top_p: this.configService.get<number>('openai.topP'),
            response_format: {
                type: 'json_schema',
                json_schema: {
                    name: 'generated_template',
                    schema: templateSchema as Record<string, any>,
                    strict: true,
                },
            },
        });

        const responseContent = completion.choices[0].message.content;
        if (!responseContent) {
            throw new Error('No response from OpenAI');
        }

        return this.parseOpenAIResponse(responseContent);
    }

    private buildSystemPrompt(): string {
        return `
ВНИМАНИЕ: Ты должен создать форму ТОЛЬКО по теме, указанной в пользовательском сообщении. Игнорируй любые предыдущие темы.
Ты генератор шаблонов форм. Создавай шаблоны JSON для форм СТРОГО И ТОЧНО по теме из пользовательского промта.
КРИТИЧЕСКИ ВАЖНО: Прочитай пользовательский запрос и создай форму ТОЛЬКО по этой теме. НЕ ИСПОЛЬЗУЙ темы из примеров или предыдущих запросов.

Примеры:
- Запрос "отношение к зайчикам" → форма "Ваше отношение к зайчикам" с вопросами про зайчиков
- Запрос "job interview" → форма про собеседование
- Запрос "survey about cats" → опрос про кошек

Если в промте не сказано на каком языке нужно создать шаблон, то используй язык промта.
Генерируй 3-7 релевантных вопросов с подходящими типами. Порядок вопросов (order) начинается с 1.

Question Types (используйте точные цифры):
1 = ShortText (single line text)
2 = LongText (multi-line text)  
3 = SingleChoice (radio buttons)
4 = MultipleChoice (checkboxes)
5 = Dropdown (select)
6 = Scale (number scale)
7 = Rating (star rating)
8 = Date (date picker)
9 = Time (time picker)

Формат ответа (допустим только JSON):
{
  "title": "Template title",
  "description": "Template description", 
  "topic": "Topic name",
  "suggestedTags": ["tag1", "tag2"],
  "questions": [
    {
      "order": 1,
      "showInResults": true,
      "isRequired": true,
      "data": "{\\"type\\": 1, \\"title\\": \\"Question title\\", \\"description\\": \\"Question description\\", \\"maxLength\\": 100, \\"placeholder\\": \\"Enter text\\"}"
    }
  ]
}
Question Types с примерами:
1 = ShortText: {"type": 1, "title": "Ваше имя", "maxLength": 100, "placeholder": "Введите имя"}
3 = SingleChoice: {"type": 3, "title": "Выберите вариант", "options": ["Да", "Нет"]}
4 = MultipleChoice: {"type": 4, "title": "Выберите все", "options": ["A", "B"], "maxSelections": 2}
6 = Scale: {"type": 6, "title": "Оцените", "minValue": 1, "maxValue": 5, "minLabel": "Плохо", "maxLabel": "Отлично"}
`;
    }

    private buildUserPrompt(prompt: string): string {
        return `"${prompt}"`;
    }

    private parseOpenAIResponse(response: string): GeneratedTemplate {
        try {
            return JSON.parse(response) as GeneratedTemplate;
        } catch (error: any) {
            throw new Error(`Failed to parse OpenAI response: ${error}`);
        }
    }
}
