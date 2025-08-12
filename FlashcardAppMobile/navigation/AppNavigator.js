import * as React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import LoginScreen from '../screens/LoginScreen';
import RegisterScreen from '../screens/RegisterScreen';
import HomeScreen from '../screens/HomeScreen';
import CreateCategoryScreen from '../screens/CreateCategoryScreen';
import AuthLoadingScreen from '../screens/AuthLoadingScreen';
import FlashcardScreen from '../screens/FlashcardScreen';

const Stack = createStackNavigator();

const AppNavigator = () => {
  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="AuthLoading" screenOptions={{ headerShown: false }}>
        <Stack.Screen name="AuthLoading" component={AuthLoadingScreen} />
        <Stack.Screen name="Login" component={LoginScreen} />
        <Stack.Screen name="Register" component={RegisterScreen} />
        <Stack.Screen name="Home" component={HomeScreen} />
        <Stack.Screen name="CreateCategory" component={CreateCategoryScreen} />
        <Stack.Screen name="Flashcards" component={FlashcardScreen} />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default AppNavigator;