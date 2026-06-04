import {Link} from "react-router-dom";
import LoginForm from "../components/LoginForm";

function LoginPage() {
    return (
        <div>
            <div className="flex justify-center">
                <LoginForm />
            </div>
        </div>
    )
}

export default LoginPage;