import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import configuration from './configs/configuration';
import { TemplateGeneratorModule } from './modules/template-generator.module';

@Module({
    imports: [
        ConfigModule.forRoot({
            isGlobal: true,
            load: [configuration],
            envFilePath: '.env.aitemplate',
        }),
        TemplateGeneratorModule,
    ],
    controllers: [AppController],
    providers: [AppService],
})
export class AppModule {}
