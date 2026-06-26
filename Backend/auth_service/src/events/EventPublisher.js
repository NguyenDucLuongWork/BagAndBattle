const amqp = require('amqplib');
require('dotenv').config();

class EventPublisher {
  constructor() {
    this.connection = null;
    this.channel = null;
    this.url = process.env.RABBITMQ_URL || 'amqp://localhost';
  }

  async connect() {
    try {
      this.connection = await amqp.connect(this.url);
      this.channel = await this.connection.createChannel();
      
      // Ensure queues exist
      await this.channel.assertQueue('auth_events', { durable: true });
      console.log('Connected to RabbitMQ and auth_events queue is ready.');
    } catch (error) {
      console.error('Failed to connect to RabbitMQ:', error.message);
    }
  }

  async publish(eventType, payload) {
    if (!this.channel) {
      console.warn(`Channel not initialized. Cannot publish event: ${eventType}`);
      return;
    }

    const message = {
      eventType,
      timestamp: new Date().toISOString(),
      payload
    };

    try {
      this.channel.sendToQueue(
        'auth_events',
        Buffer.from(JSON.stringify(message)),
        { persistent: true }
      );
      console.log(`Published event: ${eventType}`);
    } catch (error) {
      console.error(`Failed to publish event ${eventType}:`, error.message);
    }
  }
}

// Export as a singleton
const eventPublisher = new EventPublisher();
module.exports = eventPublisher;
