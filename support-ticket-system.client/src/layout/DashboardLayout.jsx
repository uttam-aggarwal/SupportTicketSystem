import Sidebar from "./Sidebar"

export default function DashboardLayout({children}){
    return(
        
        <div className="bg-gray-50 flex h-screen overflow-hidden font-sans">
            <Sidebar/>
            <h1 className="text-5xl text-red-500">TEST</h1>
            <main className="flex-1 relative bg-[#edf2f9]">
                {children}
            </main>
        </div>
    );
}