export type PowerRequestStatus = "Pending" | "Approved" | "Declined";

export type PowerRequestDto = {
    id: string;
    userId: string;
    userEmail: string;
    amount: number;
    status: PowerRequestStatus;
    createdUtc: string;
};

export type RequestPowerRequest = {
    amount: number;
};

export type ReviewPowerRequest = {
    approve: boolean;
};
