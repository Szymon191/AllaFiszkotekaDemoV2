import React, { useEffect } from 'react';
import { View, ActivityIndicator, StyleSheet } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useNavigation } from '@react-navigation/native';
import { refreshToken, validateToken } from '../services/api';

const AuthLoadingScreen = () => {
  const navigation = useNavigation();

  useEffect(() => {
    const checkAuth = async () => {
      const token = await AsyncStorage.getItem('userToken');
      if (token) {
        const isValid = await validateToken(token);
        if (isValid) {
          navigation.reset({ index: 0, routes: [{ name: 'Home' }] });
        } else {
          const payload = token.split('.')[1];
          const decodedToken = JSON.parse(atob(payload));
          const exp = decodedToken.exp * 1000;
          const now = Date.now();
          const timeLeft = exp - now;

          if (timeLeft > 0 && timeLeft <= 5 * 60 * 1000) { // Jeśli token wygaśnie w ciągu 5 minut
            try {
              const newToken = await refreshToken(token);
              await AsyncStorage.setItem('userToken', newToken);
              navigation.reset({ index: 0, routes: [{ name: 'Home' }] });
            } catch (error) {
              await AsyncStorage.removeItem('userToken');
              navigation.reset({ index: 0, routes: [{ name: 'Login' }] });
            }
          } else {
            await AsyncStorage.removeItem('userToken');
            navigation.reset({ index: 0, routes: [{ name: 'Login' }] });
          }
        }
      } else {
        navigation.reset({ index: 0, routes: [{ name: 'Login' }] });
      }
    };

    checkAuth();
  }, [navigation]);

  return (
    <View style={styles.container}>
      <ActivityIndicator size="large" color="#0000ff" />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
});

export default AuthLoadingScreen;