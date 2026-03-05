import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiClient } from './api-client.service';
import { MaterialDto, UpdateMaterialRequestDto } from '../dtos/materials';

const _PATH = '/materials';

@Injectable({ providedIn: 'root' })
export class MaterialsApi {
  constructor(private readonly api: ApiClient) {}

  listActive(): Observable<MaterialDto[]> {
    return this.api.getCached<MaterialDto[]>(_PATH);
  }

  update(id: number, dto: UpdateMaterialRequestDto): Observable<MaterialDto> {
    return this.api.patch<MaterialDto>(`${_PATH}/${id}`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_PATH);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }
}
