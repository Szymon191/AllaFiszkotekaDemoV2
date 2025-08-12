import React, { useState, useEffect } from 'react';
import { View, FlatList, Text, StyleSheet, Alert, TouchableOpacity } from 'react-native';
import { getCategories } from '../services/api';
import AsyncStorage from '@react-native-async-storage/async-storage';

const CategoryList = ({ navigation }) => {
  const [categories, setCategories] = useState([]);
  const [userId, setUserId] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        console.log('Fetching categories...');
        const token = await AsyncStorage.getItem('userToken');
        console.log('Token:', token);
        if (!token) {
          Alert.alert('Error', 'No token found. Please log in again.');
          setLoading(false);
          return;
        }

        const payload = token.split('.')[1];
        if (!payload) {
          Alert.alert('Error', 'Invalid token format.');
          setLoading(false);
          return;
        }
        const decodedToken = JSON.parse(atob(payload));
        console.log('Decoded token:', decodedToken);
        const userIdFromToken = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
        if (!userIdFromToken) {
          Alert.alert('Error', 'User ID not found in token.');
          setLoading(false);
          return;
        }
        setUserId(userIdFromToken);
        console.log('User ID:', userIdFromToken);

        const data = await getCategories(token, userIdFromToken);
        console.log('Categories data:', data);
        setCategories(data);
      } catch (error) {
        console.log('Error fetching categories:', error.message);
        Alert.alert('Error', `Failed to fetch categories: ${error.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, []);

  if (loading) {
    return (
      <View style={styles.container}>
        <Text>Loading...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Categories:</Text>
      {categories.length > 0 ? (
        <FlatList
          data={categories}
          keyExtractor={(item) => item.id.toString()}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={styles.categoryItem}
              onPress={() => navigation.navigate('Flashcards', { categoryId: item.id, categoryName: item.name })}
            >
              <Text>{item.name} (Public: {item.isPublic ? 'Yes' : 'No'})</Text>
            </TouchableOpacity>
          )}
        />
      ) : (
        <Text>No categories found.</Text>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: { padding: 20 },
  title: { fontSize: 20, fontWeight: 'bold', marginBottom: 10 },
  categoryItem: { padding: 10, borderBottomWidth: 1, borderBottomColor: '#ccc' },
});

export default CategoryList;