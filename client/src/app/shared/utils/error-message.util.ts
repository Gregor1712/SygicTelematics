import {ApiError} from '../models/apiError';

export interface ErrorResult {
  message: string;
  details?: string;
}

export function getErrorMessage(err: unknown): ErrorResult {
  if (!err || typeof err !== 'object') {
    return { message: 'Unexpected error occurred. Please try again.' };
  }

  const apiErr = err as Partial<ApiError>;
  if (apiErr.message && apiErr.message.trim().length > 0) {
    return { message: apiErr.message, details: apiErr.details };
  }

  // Handle HttpErrorResponse
  const httpError = err as {error?: unknown};
  if (typeof httpError.error === 'string' && httpError.error.trim().length > 0) {
    return { message: httpError.error };
  }

  if (httpError.error && typeof httpError.error === 'object') {
    const nested = httpError.error as {message?: string; title?: string};
    if (nested.message) return { message: nested.message };
    if (nested.title) return { message: nested.title };
  }

  return { message: 'Unexpected error occurred. Please try again.' };
}
