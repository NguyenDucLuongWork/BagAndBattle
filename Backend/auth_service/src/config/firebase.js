const admin = require('firebase-admin');
require('dotenv').config();

// Initialize Firebase Admin (for backend operations: verify tokens, create users without logging in)
let adminApp;
try {
  adminApp = admin.initializeApp({
    credential: admin.credential.cert({
      projectId: process.env.FIREBASE_PROJECT_ID,
      clientEmail: process.env.FIREBASE_CLIENT_EMAIL,
      privateKey: process.env.FIREBASE_PRIVATE_KEY ? process.env.FIREBASE_PRIVATE_KEY.replace(/\\n/g, '\n') : undefined,
    })
  });
  console.log('Firebase Admin initialized successfully.');
} catch (error) {
  console.error('Firebase Admin initialization error:', error.message);
  // We don't throw here to allow the server to start even if config is missing, 
  // but operations will fail later.
}

module.exports = {
  admin,
  adminAuth: adminApp ? admin.auth() : null
};
