import { http } from "./http";
import type {
    PowerRequestDto,
    RequestPowerRequest,
    ReviewPowerRequest,
} from "../types/power";

export async function getBalance(): Promise<number> {
    const { data } = await http.get<{ purchasePower: number }>("/power/balance");
    return data.purchasePower;
}

export async function requestPower(req: RequestPowerRequest): Promise<{ message: string }> {
    const { data } = await http.post<{ message: string }>("/power/request", req);
    return data;
}

export async function getMyPowerRequests(): Promise<PowerRequestDto[]> {
    const { data } = await http.get<PowerRequestDto[]>("/power/my");
    return data;
}

export async function getPendingPowerRequests(): Promise<PowerRequestDto[]> {
    const { data } = await http.get<PowerRequestDto[]>("/power/pending");
    return data;
}

export async function reviewPowerRequest(id: string, req: ReviewPowerRequest): Promise<{ message: string }> {
    const { data } = await http.post<{ message: string }>(`/power/${id}/review`, req);
    return data;
}
