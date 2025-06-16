import React from 'react';
import { AppLoader } from './components/AppLoader';

function App() {
  return (
      <AppLoader onRetry={() => console.log("Retry")}/> 
  );
}

export default App;
