export default function Sidebar() {
  return (
    <aside className="w-20 bg-white border-r border-gray-200 flex flex-col items-center py-6 space-y-8 z-20">
      <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center text-white font-bold text-xl">
        S
      </div>

      <nav className="flex flex-col space-y-6 text-gray-400">
        <button className="text-blue-600 p-2 bg-blue-50 rounded-lg">
          🏠
        </button>
        <button className="hover:text-blue-600 p-2">
          🎫
        </button>
        <button className="hover:text-blue-600 p-2">
          ⚙️
        </button>
      </nav>
    </aside>
  );
}