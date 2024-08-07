import { getServerSession } from "next-auth";
import { authOptions } from "../lib/auth";

export async function getSession() {
    return await getServerSession(authOptions);
}

export async function getCurrentUser() {
    try {
        const session = await getSession();

        if(!session) return null; 

        return session.user
    } catch (error) {
        return null;
    }

}