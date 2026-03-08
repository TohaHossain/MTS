export type Instrument = {
  id: string;
  symbol: string;
  name: string;
  lastPrice: number;
  maxQuantity: number;
};

export type CreateInstrumentRequest = {
  symbol: string;
  name: string;
  lastPrice: number;
};

export type CreateInstrumentResponse = {
  message: string;
};