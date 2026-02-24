export const CanAssignTickets = (role)=> role==="Admin";
export const canUpdateStatus = (role)=> role==="Admin"||role==="Agent";
export const CanCreateTicket = (role)=>
role==="Customer";
export const CanDeleteTicket = (role)=>
role==="Admin";