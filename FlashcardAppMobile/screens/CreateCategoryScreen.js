import React from 'react';
import { View } from 'react-native';
import CreateCategoryForm from '../components/CreateCategoryForm';

const CreateCategoryScreen = ({ navigation }) => {
  return (
    <View>
      <CreateCategoryForm navigation={navigation} />
    </View>
  );
};

export default CreateCategoryScreen;