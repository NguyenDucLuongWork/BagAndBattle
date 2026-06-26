const express = require('express');
const authController = require('../controllers/AuthController');
const { verifyToken, requireAuth } = require('../middleware/authMiddleware');

const router = express.Router();

// Public routes
router.post('/register', authController.register);
router.post('/login', authController.login);
router.post('/login-google', authController.loginWithGoogle);

// Protected routes
router.post('/logout', verifyToken, authController.logout);
router.get('/me', verifyToken, requireAuth, authController.getCurrentUser);

module.exports = router;
