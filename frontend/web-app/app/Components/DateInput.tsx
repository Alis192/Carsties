import React from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import DatePicker, { DatePickerProps } from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

type Props = {
    label: string;
    type?: string;
    showLabel?: boolean;
} & UseControllerProps & Partial<DatePicker>;

export default function DateInput(props: Props) {
    const { fieldState, field } = useController({ ...props, defaultValue: '' });

    return (
        <div className='block'>
            <DatePicker
                {...props}
                {...field}
                onChange={(value: Date | null) => field.onChange(value)}
                selected={field.value}
                placeholderText={props.label}
                className={`
                    rounded-lg w-[100%] flex flex-col
                    ${fieldState.error
                        ? 'bg-red-50 border-red-500 text-red-900'
                        : (!fieldState.invalid && fieldState.isDirty)
                            ? 'bg-green-50 border-green-500 text-green-900'
                            : ''
                    }
                `}
            />
            {fieldState.error && (
                <div className='text-red-500 text-sm'>{fieldState.error.message}</div>
            )}
        </div>
    );
}