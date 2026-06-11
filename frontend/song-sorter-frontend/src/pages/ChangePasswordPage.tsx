import {Link} from "react-router-dom";
import ChangePasswordForm from "../components/ChangePasswordForm";

function ChangePasswordPage() {
    return (
        <div>
            <div className="flex justify-center">
                <ChangePasswordForm />
            </div>
        </div>
    )
}

export default ChangePasswordPage;