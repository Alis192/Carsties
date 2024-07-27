'use server'

import { Auction, PagedResult } from "@/types"

export async function getData(pageNumber: number, pageSize: number) : Promise<PagedResult<Auction>> {
    const res = await fetch(`http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`)
  
    if (!res.ok) throw new Error('Failed to fetch the data')
  
    return res.json()
}