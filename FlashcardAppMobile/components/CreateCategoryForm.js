import React, { useState } from 'react';
import { View, TextInput, Button, Alert, StyleSheet } from 'react-native';
import { createCategory } from '../services/api';
import AsyncStorage from '@react-native-async-storage/async-storage';

const CreateCategoryForm = ({ navigation }) => {
  const [name, setName] = useState('');
  const [isPublic, setIsPublic] = useState(false);

  const handleCreateCategory = async () => {
    try {
      if (!name.trim()) {
        Alert.alert('Error', 'Name is required.');
        return;
      }

      const token = await AsyncStorage.getItem('userToken');
      if (!token) {
        Alert.alert('Error', 'No token found. Please log in again.');
        return;
      }

      const payload = token.split('.')[1];
      const decodedToken = JSON.parse(atob(payload));
      const userId = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

      await createCategory(token, { userId, name, isPublic });
      Alert.alert('Success', 'Category created successfully!');
      navigation.navigate('Home'); // Powrót do HomeScreen po stworzeniu
    } catch (error) {
      Alert.alert('Error', `Failed to create category: ${error.message}`);
    }
  };

  return (
    <View style={styles.container}>
      <TextInput
        style={styles.input}
        placeholder="Category Name"
        value={name}
        onChangeText={setName}
      />
      <Button
        title={isPublic ? 'Public' : 'Private'}
        onPress={() => setIsPublic(!isPublic)}
      />
      <Button title="Create Category" onPress={handleCreateCategory} />
      <Button title="Back" onPress={() => navigation.goBack()} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: { padding: 20 },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    padding: 10,
    marginBottom: 10,
    borderRadius: 5,
  },
});

export default CreateCategoryForm;