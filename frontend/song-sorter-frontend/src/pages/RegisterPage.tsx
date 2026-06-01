import {Link} from "react-router-dom";

function RegisterPage() {
    return (
        <div>
            <h1>Register for the Music App</h1>
            <p>Please <Link to="/register">register</Link> or <Link to="/login">login</Link> to continue.</p>
        </div>
    )
}

export default RegisterPage;