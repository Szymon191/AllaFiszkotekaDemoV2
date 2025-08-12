import React, { useState } from 'react';
import { View, TextInput, Button, Alert, StyleSheet } from 'react-native';
import { createFlashcard } from '../services/api';
import AsyncStorage from '@react-native-async-storage/async-storage';

const FlashcardForm = ({ navigation, categoryId, categoryName }) => {
  const [word, setWord] = useState('');
  const [translation, setTranslation] = useState('');
  const [exampleUsage, setExampleUsage] = useState('');
  const [tags, setTags] = useState(''); 
  const [difficulty, setDifficulty] = useState(''); // Dodano difficulty

  const handleCreateFlashcard = async () => {
    try {
      if (!word.trim() || !translation.trim()) {
        Alert.alert('Error', 'Both question and answer are required.');
        return;
      }

      const token = await AsyncStorage.getItem('userToken');
      if (!token) {
        Alert.alert('Error', 'No token found. Please log in again.');
        return;
      }

      await createFlashcard(token, { categoryId, word, translation, exampleUsage, tags: tags.split(','), difficulty });
      Alert.alert('Success', 'Flashcard created successfully!');
      navigation.navigate('Home'); // Powrót do HomeScreen po stworzeniu
    } catch (error) {
      Alert.alert('Error', `Failed to create flashcard: ${error.message}`);
    }
  };

  return (
    <View style={styles.container}>
      <TextInput
        style={styles.input}
        placeholder="word"
        value={word}
        onChangeText={setWord}
      />
      <TextInput
        style={styles.input}
        placeholder="translation"
        value={translation}
        onChangeText={setTranslation}
      />
        <TextInput
        style={styles.input}
        placeholder="Example Usage"
        value={exampleUsage}
        onChangeText={setExampleUsage}
        />
        <TextInput
        style={styles.input}
        placeholder="Tags (comma separated)"
        value={tags}
        onChangeText={setTags}  
        />
        <TextInput
        style={styles.input}
        placeholder="Difficulty (easy, medium, hard)"
        value={difficulty}
        onChangeText={setDifficulty}
        />
      <Button title="Create Flashcard" onPress={handleCreateFlashcard} />
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

export default FlashcardForm;