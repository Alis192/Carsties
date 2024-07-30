'use server'

import { fetchWrapper } from "@/lib/fetchWrapper"
import { Auction, PagedResult } from "@/types"
import { NextApiRequest } from "next"
import { getToken } from "next-auth/jwt"
import { cookies, headers } from "next/headers"
import { FieldValues } from "react-hook-form"

export async function getData(query: string) : Promise<PagedResult<Auction>> {
    return await fetchWrapper.get(`search/${query}`)
}


export async function  updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1
    }

    return await fetchWrapper.put('auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', data);
}

export async function createAuction(data: FieldValues) {
    return await fetchWrapper.post('auctions', data);
}

export async function getTokenWorkaround() {
    const req = {
        headers: Object.fromEntries(headers() as Headers),
        cookies: Object.fromEntries(
            cookies()
                .getAll()
                .map(c => [c.name, c.value])
        )
    } as NextApiRequest

    return await getToken({req})
}