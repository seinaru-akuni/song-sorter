import {Link} from "react-router-dom";
import LoginForm from "../components/LoginForm";

function LoginPage() {
    return (
        <div>
            <h1>Login to the Music App</h1>
            <LoginForm />
            <p>Please <Link to="/register">register</Link> or <Link to="/login">login</Link> to continue.</p>
        </div>
    )
}

export default LoginPage;