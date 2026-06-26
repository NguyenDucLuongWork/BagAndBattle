const authService = require('../services/AuthService');
const { z } = require('zod');

// Validation schemas
const registerSchema = z.object({
  email: z.string().email("Invalid email format"),
  password: z.string().min(6, "Password must be at least 6 characters")
});

const loginSchema = z.object({
  idToken: z.string().min(1, "Firebase ID Token is required")
});

const googleLoginSchema = z.object({
  idToken: z.string().min(1, "Google ID Token is required")
});

class AuthController {
  async register(req, res) {
    try {
      const { email, password } = registerSchema.parse(req.body);
      const user = await authService.Register(email, password);
      res.status(201).json({ message: "User registered successfully", uid: user.uid });
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(400).json({ error: error.message });
    }
  }

  async login(req, res) {
    try {
      const { idToken } = loginSchema.parse(req.body);
      const result = await authService.Login(idToken);
      res.status(200).json(result);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(401).json({ error: error.message });
    }
  }

  async loginWithGoogle(req, res) {
    try {
      const { idToken } = googleLoginSchema.parse(req.body);
      const result = await authService.LoginWithGoogle(idToken);
      res.status(200).json(result);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(401).json({ error: error.message });
    }
  }

  async logout(req, res) {
    try {
      // Assuming a middleware sets req.user from the bearer token
      const uid = req.user ? req.user.uid : null;
      await authService.Logout(uid);
      res.status(200).json({ message: "Logged out successfully" });
    } catch (error) {
      res.status(500).json({ error: error.message });
    }
  }

  async getCurrentUser(req, res) {
    try {
      const authHeader = req.headers.authorization;
      if (!authHeader || !authHeader.startsWith('Bearer ')) {
        return res.status(401).json({ error: "Missing or invalid authorization header" });
      }

      const idToken = authHeader.split('Bearer ')[1];
      const user = await authService.GetCurrentUser(idToken);

      if (!user) {
        return res.status(401).json({ error: "Invalid token or user not found" });
      }

      res.status(200).json({ user });
    } catch (error) {
      res.status(500).json({ error: error.message });
    }
  }
}

module.exports = new AuthController();
