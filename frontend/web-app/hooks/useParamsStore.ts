import { createWithEqualityFn } from "zustand/traditional"

type State = {
    pageNumber: number
    pageSize: number
    pageCount: number
    searchTerm: string
    searchValue: string
    orderBy: string
    filterBy: string
    seller?: string
    winner?: string
}


type Actions = {
    setParams: (params: Partial<State>) => void
    reset: () => void
    setSearchValue: (value: string) => void
}

const InitialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    searchValue: '',
    orderBy: 'make',
    filterBy: 'live',
    seller: undefined,
    winner: undefined
}

export const useParamsStore = createWithEqualityFn<State & Actions>()((set) => ({
    ...InitialState,

    setParams: (newParams: Partial<State>) => {
        set((state) => {
            if (newParams.pageNumber) {
                return{ ...state, pageNumber: newParams.pageNumber}
            } else {
                return {...state, ...newParams, pageNumber: 1}
            }
        })
    },

    reset: () => set(InitialState),

    setSearchValue: (value: string) => {
        set({searchValue: value})
    }
}))