import React from 'react';
import { View, Button } from 'react-native';
import CategoryList from '../components/CategoryList';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { logout } from '../services/api';

const HomeScreen = ({ navigation }) => {
  const handleLogout = async () => {
    const token = await AsyncStorage.getItem('userToken');
    if (token) {
      try {
        await logout(token);
      } catch (error) {
        console.log('Logout error:', error.message);
      }
    }
    await AsyncStorage.removeItem('userToken');
    navigation.reset({
      index: 0,
      routes: [{ name: 'Login' }],
    });
  };

  return (
    <View>
      <CategoryList navigation={navigation} />
      <Button
        title="Create New Category"
        onPress={() => navigation.navigate('CreateCategory')}
      />
      <Button title="Logout" onPress={handleLogout} color="red" />
    </View>
  );
};

export default HomeScreen;