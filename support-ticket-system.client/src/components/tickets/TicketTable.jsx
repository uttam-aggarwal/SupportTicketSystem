export default function TicketTable() {
  return (
    <>
      <div className="p-6 border-b border-gray-100 flex justify-between items-center">
        <h2 className="text-xl font-semibold text-gray-800">
          My Tickets
        </h2>

        <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">
          + New Ticket
        </button>
      </div>

      <div className="overflow-y-auto flex-1">
        <table className="w-full text-left">
          <thead>
            <tr className="text-xs text-gray-400 border-b border-gray-100">
              <th className="px-6 py-4 font-medium">Ticket</th>
              <th className="px-6 py-4 font-medium">Status</th>
              <th></th>
            </tr>
          </thead>

          <tbody>
            <tr className="border-b hover:bg-gray-50 cursor-pointer">
              <td className="px-6 py-4">
                <p className="font-semibold text-gray-800">
                  Support ticket 1
                </p>
                <p className="text-xs text-gray-400">
                  Last modified 2 hours ago
                </p>
              </td>

              <td className="px-6 py-4">
                <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full">
                  Resolved
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </>
  );
}