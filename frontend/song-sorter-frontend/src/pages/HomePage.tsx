import {Link} from "react-router-dom";


function HomePage() {
    return (
        <div>
            <h1>Welcome to the Music App</h1>
            <h1 className="text-4xl font-bold text-blue-500">
                Tailwind працює!
            </h1>
            <p>Please <Link to="/register">register</Link> or <Link to="/login">login</Link> to continue.</p>
        </div>
    )
}

export default HomePage;