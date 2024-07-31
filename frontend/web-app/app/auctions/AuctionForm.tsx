'use client'

import { Button, TextInput } from 'flowbite-react';
import React, { useEffect } from 'react'
import { FieldValues, useForm } from 'react-hook-form'
import Input from '../Components/input';
import DateInput from '../Components/DateInput';
import { createAuction } from '../actions/auctionActions';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';

export default function AuctionForm() {
  const router = useRouter();
  const { control, handleSubmit, setFocus, formState: { isSubmitting, isValid } } = useForm({
    mode: 'onTouched'
  });


  useEffect(() => {
    setFocus('')
  }, [setFocus])

  async function onSubmit(data: FieldValues) {
    try {
      const res = await createAuction(data);
      if (res.error) {
        throw res.error
      }
      router.push(`/auctions/details/${res.id}`)
    }
    catch (error : any) {
      toast.error(error.status + ' ' + error.message)
    }
  }

  return (
    <form className='flex flex-col mt-3' onSubmit={handleSubmit(onSubmit)}>
      <Input label='Make' name='make' control={control} rules={{ required: 'Make is required' }} />
      <Input label='Model' name='model' control={control} rules={{ required: 'Model is required' }} />
      <Input label='Color' name='color' control={control} rules={{ required: 'Color is required' }} />

      <div className='grid grid-cols-2 gap-3'>
        <Input label='Year' name='year' control={control} type='number' rules={{ required: 'Year is required' }} />
        <Input label='Mileage' name='mileage' control={control} type='number' rules={{ required: 'Mileage is required' }} />
      </div>

      <Input label='Image URL' name='imageUrl' control={control} rules={{ required: 'Image URL is required' }} />

      <div className='grid grid-cols-2 gap-3'>
        <Input label='Reserve Price (enter 0 if no reserve' name='reservePrice' control={control} type='number' rules={{ required: 'Reserve price  is required' }} />
        <DateInput label='Auction end date/time' name='auctionEnd' control={control} dateFormat='dd MMMM yyyy h:mm a' showTimeSelect rules={{ required: 'Auction end date is required' }} />
      </div>

      <div className='flex justify-between'>
        <Button outline color='gray'>Cancel</Button>
        <Button isProcessing={isSubmitting} disabled={!isValid} type='submit' outline color='success'>Submit</Button>
      </div>
    </form>
  )
}
