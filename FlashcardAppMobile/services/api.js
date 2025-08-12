import axios from 'axios';

const API_URL = 'http://localhost:5018/api';

export const login = async (email, password) => {
  try {
    const response = await axios.post(`${API_URL}/auth/login`, { email, password });
    return response.data.token;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Login failed');
  }
};

export const register = async (email, nickname, password) => {
  try {
    const response = await axios.post(`${API_URL}/auth/register`, { email, nickname, password });
    return response.data.token;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Registration failed');
  }
};

export const getCategories = async (token, userId) => {
  try {
    console.log(`Fetching categories for user ${userId} with token ${token.slice(0, 10)}...`);
    const response = await axios.get(`${API_URL}/categories/user/${userId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    console.log('Categories response:', response.data);
    return response.data;
  } catch (error) {
    console.log('Categories error:', error.response?.status, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Failed to fetch categories');
  }
};

export const createCategory = async (token, { userId, name, isPublic }) => {
  try {
    console.log(`Creating category for user ${userId} with name ${name}...`);
    const response = await axios.post(
      `${API_URL}/categories`,
      { userId, name, isPublic },
      { headers: { Authorization: `Bearer ${token}` } }
    );
    console.log('Category created:', response.data);
    return response.data;
  } catch (error) {
    console.log('Category creation error:', error.response?.status, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Failed to create category');
  }
};

export const refreshToken = async (oldToken) => {
  try {
    console.log('Refreshing token...');
    const response = await axios.post(
      `${API_URL}/auth/refresh`,
      {},
      { headers: { Authorization: `Bearer ${oldToken}` } }
    );
    console.log('New token:', response.data.token);
    return response.data.token;
  } catch (error) {
    console.log('Token refresh error:', error.response?.status, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Failed to refresh token');
  }
};

export const validateToken = async (token) => {
  try {
    console.log('Validating token...');
    const response = await axios.post(
      `${API_URL}/auth/validate`,
      {},
      { headers: { Authorization: `Bearer ${token}` } }
    );
    console.log('Validation response:', response.data);
    return response.data.valid;
  } catch (error) {
    console.log('Token validation error:', error.response?.status, error.response?.data || error.message);
    return false;
  }
};

export const logout = async (token) => {
  try {
    console.log('Logging out...');
    const response = await axios.post(
      `${API_URL}/auth/logout`,
      {},
      { headers: { Authorization: `Bearer ${token}` } }
    );
    console.log('Logout response:', response.data);
    return response.data;
  } catch (error) {
    console.log('Logout error:', error.response?.status, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Failed to logout');
  }
};

export const createFlashcard = async (token, { categoryId, word, translation, exampleUsage, tags, difficulty }) => {
  try {
    console.log(`Creating flashcard for category ${categoryId}...`);
    const response = await axios.post(
      `${API_URL}/flashcards`,
      { categoryId, word, translation, exampleUsage, tags, difficulty},
      { headers: { Authorization: `Bearer ${token}` } }
    );
    console.log('Flashcard created:', response.data);
    return response.data;
  } catch (error) {
    console.log('Flashcard creation error:', error.response?.status, error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Failed to create flashcard');
  }
};