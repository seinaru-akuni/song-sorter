import {Link} from "react-router-dom";
import RegisterForm from "../components/RegisterForm";

function RegisterPage() {
    return (
        <div>
            <div className="flex justify-center">
                <RegisterForm />
            </div>
        </div>
    )
}

export default RegisterPage;