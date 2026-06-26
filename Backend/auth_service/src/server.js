const express = require('express');
const cors = require('cors');
const authRoutes = require('./routes/authRoutes');
const eventPublisher = require('./events/EventPublisher');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(express.json());

// Routes
app.use('/api/auth', authRoutes);

// Health check endpoint
app.get('/health', (req, res) => {
  res.status(200).json({ status: 'OK', message: 'Auth service is running' });
});

// Initialize Event Publisher and start server
const startServer = async () => {
  try {
    await eventPublisher.connect();
    
    app.listen(PORT, () => {
      console.log(`Auth Service is running on http://localhost:${PORT}`);
    });
  } catch (error) {
    console.error('Failed to start server:', error);
    process.exit(1);
  }
};

startServer();
