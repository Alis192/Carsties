import EmptyFilter from '@/app/Components/EmptyFilter'
import React from 'react'

export default function Page({searchParams}: {searchParams: {callbackUrl: string}}) {
  return (
    <EmptyFilter 
      title='You need to be logged in to perform that action'
      subtitle='Please click below to sign in'
      showLogin
      callbackUrl={searchParams.callbackUrl}
    />
  )
}
