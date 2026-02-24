export default function ChatWidget() {
  return (
    <div className="absolute bottom-12 right-16 w-80 bg-white rounded-t-xl rounded-b-lg shadow-2xl border border-gray-100 flex flex-col z-30">
      <div className="bg-blue-600 text-white p-3 rounded-t-xl flex justify-between items-center">
        <span className="text-sm font-medium">
          Chat with Agent
        </span>
      </div>

      <div className="h-64 p-4 overflow-y-auto bg-gray-50 text-sm">
        <div className="bg-white p-2 rounded mb-2">
          Hello! How can I help?
        </div>

        <div className="bg-blue-600 text-white p-2 rounded self-end">
          I need help with my ticket.
        </div>
      </div>

      <div className="p-3 border-t flex">
        <input
          className="flex-1 text-sm outline-none"
          placeholder="Type a message..."
        />
      </div>
    </div>
  );
}