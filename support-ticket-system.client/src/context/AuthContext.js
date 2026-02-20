import {createContext,useContext} from "react";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const role =localStorage.getItem("role");

    return(
        <AuthContext.Provider value={{ role }}>
            {children}
        </AuthContext.Provider>
    );
}
export const useAuth = () => useContext(AuthContext);