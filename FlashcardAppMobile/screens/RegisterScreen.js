import React from 'react';
import { View } from 'react-native';
import RegisterForm from '../components/RegisterForm';

const RegisterScreen = ({ navigation }) => {
  return (
    <View>
      <RegisterForm navigation={navigation} />
    </View>
  );
};

export default RegisterScreen;