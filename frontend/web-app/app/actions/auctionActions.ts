'use server'

import { Auction, PagedResult } from "@/types"
import { NextApiRequest } from "next"
import { getToken } from "next-auth/jwt"
import { cookies, headers } from "next/headers"

export async function getData(query: string) : Promise<PagedResult<Auction>> {
    const res = await fetch(`http://localhost:6001/search${query}`)
  
    if (!res.ok) throw new Error('Failed to fetch the data')
  
    return res.json()
}


export async function  UpdateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1
    }

    const token = await getTokenWorkaround();

    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {
            'Content-type': 'application/json',
            'Authorization': 'Bearer ' + token?.access_token
        },
        body: JSON.stringify(data)
    })

    if(!res.ok) return {status: res.status, message: res.statusText}

    return res.statusText
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