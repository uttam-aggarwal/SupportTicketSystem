import DashboardLayout from "../layout/DashboardLayout";
import TicketTable from "../components/tickets/TicketTable";
import ChatWidget from "../components/chat/ChatWidget";

export default function Dashboard() {
  return (
    <DashboardLayout>
      <div className="bg-blue-600 h-64 w-full p-8 flex justify-between items-start text-white relative">
        <div>
          <p className="text-sm text-blue-200 mb-1">Customer Panel</p>
          <h1 className="text-3xl font-semibold">Dashboard</h1>
        </div>
      </div>

      <div className="absolute top-32 left-8 right-8 bottom-8 bg-white rounded-xl shadow-sm border border-gray-100 flex flex-col z-10 overflow-hidden">
        <TicketTable />
      </div>

      <ChatWidget />
    </DashboardLayout>
  );
}