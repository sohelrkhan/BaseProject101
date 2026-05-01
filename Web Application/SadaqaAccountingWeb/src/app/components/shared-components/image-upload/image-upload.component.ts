import { Component, EventEmitter, input, Input, Output } from "@angular/core";
import { CommonModule } from "@angular/common";
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators
} from "@angular/forms";

@Component({
  selector: "app-image-upload",
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: "./image-upload.component.html",
  styleUrl: "./image-upload.component.scss"
})
export class ImageUploadComponent {
  imageForm: FormGroup;
  previewUrl: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;

  @Input() entityId!: number;
  @Input() entityType?: string; // optional
  @Input() isProfilePictureUpload: boolean = false;
  @Input() isItemPictureUpload: boolean = false;
  @Input() isDefaultUpload: boolean = false;
  @Output() imageUploaded = new EventEmitter<{
    file: File;
    entityId: number;
    entityType?: string;
  }>();

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.imageForm = this.fb.group({
      imageFile: [null, Validators.required]
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];

    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      const file = input.files[0];
      this.imageForm.patchValue({ imageFile: file });

      const reader = new FileReader();
      reader.onload = () => (this.previewUrl = reader.result);
      reader.readAsDataURL(file);
    }
  }

  uploadImage() {
    if (this.selectedFile && this.imageForm.valid) {
      this.imageUploaded.emit({
        file: this.selectedFile,
        entityId: this.entityId,
        entityType: this.entityType
      });
    }
  }
}
