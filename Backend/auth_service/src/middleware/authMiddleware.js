const { adminAuth } = require('../config/firebase');

const verifyToken = async (req, res, next) => {
  const authHeader = req.headers.authorization;
  if (!authHeader || !authHeader.startsWith('Bearer ')) {
    // If not authenticated, we just proceed without req.user for logout/public routes
    return next();
  }

  const idToken = authHeader.split('Bearer ')[1];

  try {
    if (adminAuth) {
      const decodedToken = await adminAuth.verifyIdToken(idToken);
      req.user = decodedToken;
    }
    next();
  } catch (error) {
    // Ignore invalid tokens here and let specific routes handle it or require auth
    next();
  }
};

const requireAuth = (req, res, next) => {
  if (!req.user) {
    return res.status(401).json({ error: "Unauthorized. Please log in." });
  }
  next();
};

module.exports = {
  verifyToken,
  requireAuth
};
