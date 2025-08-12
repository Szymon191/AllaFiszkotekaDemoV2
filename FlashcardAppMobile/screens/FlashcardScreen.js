import React from 'react';
import { View } from 'react-native';
import FlashcardForm from '../components/FlashcardForm';

const FlashcardScreen = ({ route, navigation }) => {
  const { categoryId, categoryName } = route.params;

  return (
    <View>
      <FlashcardForm navigation={navigation} categoryId={categoryId} categoryName={categoryName} />
    </View>
  );
};

export default FlashcardScreen;