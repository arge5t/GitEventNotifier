## GitEvent Notifier

Проект, демонстрирующий асинхронную обработку событий из GitHub с использованием RabbitMQ и .NET.

## Цель проекта

Целью проекта является изучение и практика работы с очередями сообщений на примере RabbitMQ.  
Проект реализует архитектуру "Producer-Consumer", где:

- **Producer** собирает события из GitHub API и публикует их в очередь RabbitMQ.
- **Consumer** читает сообщения из очереди и обрабатывает их (в данном случае — логирует в консоль и файл).

## Структура проекта
src/
├── GitEventNotifier.Common/          
├── GitEventNotifier.Infrastructure/  
├── GitEventNotifier.Producer/        
└── GitEventNotifier.Consumer/   

- **Producer** — фоновая служба, которая опрашивает GitHub API и отправляет события в очередь.
- **Consumer** — фоновая служба, которая получает и обрабатывает сообщения из очереди.
- **Инфраструктура** — общие компоненты: интерфейсы, сериализация, клиенты для взаимодействия с RabbitMQ.

## Технологии

- **.NET 8**
- **RabbitMQ.Client** (для взаимодействия с RabbitMQ)
- **Octokit** (для работы с GitHub API)
- **BackgroundService** (.NET)
- **Dependency Injection** (.NET)

## Запуск

1. Убедитесь, что установлен Docker.
2. Запустите RabbitMQ:

   ```bash
   docker-compose up -d
   ```
   ```JSON
   {
      "GitHub": {
        "Username": "your-github-username"
      }
   }
   ```
## Возможности для расширения 

    Поддержка других типов событий (Pull Request, Tag).
    Интеграция с GitHub Webhooks.
    Отправка уведомлений (например, в Slack или по email).
    Использование кастомных сериализаторов (MessagePack, Protobuf).
    Настройка durable очередей и отказоустойчивости.
     
