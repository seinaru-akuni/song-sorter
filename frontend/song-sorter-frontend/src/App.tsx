import { useEffect, useState } from 'react'
import './App.css'
import { useGoogleLogin } from '@react-oauth/google';

import GoogleLoginButton from './components/GoogleLoginButton';
import PlaylistsList from './components/PlaylistsList';

import RegisterForm from './components/RegisterForm';
import LoginForm from './components/LoginForm';

function App() {
  
  

  return (
    <div>
      <RegisterForm />
      <LoginForm />
    </div>
  )
}

export default App