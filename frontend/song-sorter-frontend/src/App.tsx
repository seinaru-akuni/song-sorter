import { useEffect, useState } from 'react'
import './App.css'
import { useGoogleLogin } from '@react-oauth/google';

import GoogleLoginButton from './components/GoogleLoginButton';
import PlaylistsList from './components/PlaylistsList';

function App() {
  
  

  return (
    <div>
      <GoogleLoginButton />
      <PlaylistsList />
    </div>
  )
}

export default App