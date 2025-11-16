import React, { useContext } from 'react'
import { AuthContext } from "../../context/AuthContext";
import '../topbar.css'

function UserLogout() {
    const { authlogout, username } = useContext(AuthContext);
    async function logout() {
        await authlogout();
    }
    const handleSubmit = (e) => {
        e.preventDefault();
        logout();
    }

    return (
        <div className="usermenu" role="none">
            <div className="usermenu__header">Signed in as <strong>{username}</strong></div>
            <button className="btn btn--danger" onClick={handleSubmit} role="menuitem">
                Logout
            </button>
        </div>
    );
}
export default UserLogout;