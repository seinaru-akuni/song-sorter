import { useEffect, useState } from 'react'
import './App.css'
import { useGoogleLogin } from '@react-oauth/google';

import GoogleLoginButton from './components/GoogleLoginButton';


function App() {
  
  

  return (
    <div>
      <GoogleLoginButton />
    </div>
  )
}

export default App